import { MOCK_CONFLICT_STATUS, MOCK_LATENCY_MS, MOCK_NOT_FOUND_STATUS, MOCK_VALIDATION_STATUS } from '@/constants/api';
import { DEFAULT_NEW_SITE_STATUS } from '@/constants/domain';
import type { CapacitySummary, CreateSiteRequest, SiteDetail, SiteFilter, SiteSummary, Tenancy, TowerConfiguration } from '@/types';
import { ApiError } from '../ApiError';
import type { OperatorsApi, SitesApi, TowersApi } from '../contracts';
import { MOCK_OPERATORS, MOCK_SITES, MOCK_TENANCIES, MOCK_TOWERS } from './mockData';
import type { MockSite, MockTenancy, MockTower } from './mockData';

const PERCENT = 100;

export function delay<T>(produce: () => T): Promise<T> {
    return new Promise((resolve, reject) => setTimeout(() => {
        try { resolve(produce()); } catch (e) { reject(e); }
    }, MOCK_LATENCY_MS));
}

export function isActive(endDate: string | null): boolean {
    return endDate === null || new Date(endDate) > new Date();
}

function towerFor(siteId: number): MockTower {
    return MOCK_TOWERS.find(t => t.siteId === siteId)!;
}

export function capacityOf(tower: MockTower): CapacitySummary {
    const activeTenantCount = MOCK_TENANCIES.filter(t => t.towerId === tower.id && isActive(t.endDate)).length;
    const availableSlots = tower.maxTenants - activeTenantCount;
    const utilisationPercent = Math.round((activeTenantCount / tower.maxTenants) * PERCENT);
    return { maxTenants: tower.maxTenants, activeTenantCount, availableSlots, utilisationPercent, hasCapacity: availableSlots > 0 };
}

function toSummary(site: MockSite): SiteSummary {
    const tower = towerFor(site.id);
    const { id, siteCode, name, countryCode, city, status } = site;
    return { id, siteCode, name, countryCode, city, status, towerId: tower.id, capacity: capacityOf(tower) };
}

function matches(site: MockSite, filter: SiteFilter): boolean {
    const text = filter.search?.toLowerCase();
    const textHit = !text || [site.siteCode, site.name, site.city].some(v => v.toLowerCase().includes(text));
    return textHit && (!filter.country || site.countryCode === filter.country) && (!filter.status || site.status === filter.status);
}

function toTenancy(tenancy: MockTenancy): Tenancy {
    const operatorName = MOCK_OPERATORS.find(o => o.id === tenancy.operatorId)?.name ?? '';
    const { id, operatorId, startDate, endDate, monthlyRent, currency } = tenancy;
    return { id, operatorId, operatorName, startDate, endDate, monthlyRent, currency };
}

function toDetail(site: MockSite): SiteDetail {
    const { id, towerType, heightMetres, maxTenants } = towerFor(site.id);
    const tenancies = MOCK_TENANCIES.filter(t => t.towerId === id).map(toTenancy);
    const tower = { id, towerType, heightMetres, maxTenants };
    return { ...toSummary(site), latitude: site.latitude, longitude: site.longitude, tower, tenancies };
}

function findSite(id: number): MockSite {
    const site = MOCK_SITES.find(s => s.id === id);
    if (!site) throw new ApiError(`Site ${id} was not found.`, MOCK_NOT_FOUND_STATUS);
    return site;
}

function toConfiguration(tower: MockTower): TowerConfiguration {
    const site = findSite(tower.siteId);
    return {
        towerId: tower.id, siteId: site.id, siteCode: site.siteCode, siteName: site.name,
        countryCode: site.countryCode, siteStatus: site.status, towerType: tower.towerType,
        heightMetres: tower.heightMetres, capacity: capacityOf(tower),
    };
}

function validateCreate(request: CreateSiteRequest): void {
    if (MOCK_SITES.some(s => s.siteCode === request.siteCode)) {
        throw new ApiError(`Site code '${request.siteCode}' already exists.`, MOCK_CONFLICT_STATUS);
    }
    if (request.heightMetres <= 0 || request.maxTenants <= 0) {
        throw new ApiError('Tower height and max tenants must be greater than zero.', MOCK_VALIDATION_STATUS);
    }
}

function nextId(ids: number[]): number {
    return Math.max(0, ...ids) + 1;
}

// Mirrors POST /api/sites: new site starts Planned with one tower and no tenancies.
function createSite(request: CreateSiteRequest): SiteDetail {
    validateCreate(request);
    const { siteCode, name, countryCode, city, latitude, longitude, towerType, heightMetres, maxTenants } = request;
    const site: MockSite = { id: nextId(MOCK_SITES.map(s => s.id)), siteCode, name, countryCode, city, latitude, longitude, status: DEFAULT_NEW_SITE_STATUS };
    MOCK_SITES.push(site);
    MOCK_TOWERS.push({ id: nextId(MOCK_TOWERS.map(t => t.id)), siteId: site.id, towerType, heightMetres, maxTenants });
    return toDetail(site);
}

export const mockSitesApi: SitesApi = {
    list: (filter = {}) => delay(() => MOCK_SITES.filter(s => matches(s, filter)).map(toSummary)),
    get: id => delay(() => toDetail(findSite(id))),
    create: request => delay(() => createSite(request)),
};

export const mockOperatorsApi: OperatorsApi = {
    list: () => delay(() => [...MOCK_OPERATORS]),
};

export const mockTowersApi: TowersApi = {
    list: () => delay(() => MOCK_TOWERS.map(toConfiguration)),
};
