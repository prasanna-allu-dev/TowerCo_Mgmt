import { MS_PER_DAY, RENEWAL_WINDOW_DAYS, SITE_STATUSES } from '@/constants/domain';
import type {
    CountryUtilisation, CurrencyAmount, ExpiringTenancy, OperatorShare, PortfolioDashboard,
    PortfolioKpis, SellableTower, SiteStatus,
} from '@/types';
import type { DashboardApi } from '../contracts';
import { buildAlerts } from './mockAlerts';
import { capacityOf, delay, isActive } from './mockApi';
import { MOCK_OPERATORS, MOCK_SITES, MOCK_TENANCIES, MOCK_TOWERS } from './mockData';
import type { MockSite, MockTenancy } from './mockData';

const PERCENT = 100;

const percentOf = (part: number, total: number) => (total ? Math.round((part / total) * PERCENT) : 0);
const activeTenancies = () => MOCK_TENANCIES.filter(t => isActive(t.endDate));
const siteOfTower = (towerId: number) => MOCK_SITES.find(s => s.id === MOCK_TOWERS.find(t => t.id === towerId)?.siteId)!;

function towersOf(sites: MockSite[]) {
    const ids = new Set(sites.map(s => s.id));
    return MOCK_TOWERS.filter(t => ids.has(t.siteId)).map(capacityOf);
}

function buildKpis(): PortfolioKpis {
    const capacities = towersOf(MOCK_SITES);
    const totalSlots = capacities.reduce((sum, c) => sum + c.maxTenants, 0);
    const occupiedSlots = capacities.reduce((sum, c) => sum + c.activeTenantCount, 0);
    return {
        totalSites: MOCK_SITES.length,
        activeSites: MOCK_SITES.filter(s => s.status === 'Active').length,
        totalSlots, occupiedSlots,
        utilisationPercent: percentOf(occupiedSlots, totalSlots),
        fullTowers: capacities.filter(c => !c.hasCapacity).length,
        emptyTowers: capacities.filter(c => c.activeTenantCount === 0).length,
    };
}

function buildSitesByStatus(): Record<SiteStatus, number> {
    const entries = SITE_STATUSES.map(status => [status, MOCK_SITES.filter(s => s.status === status).length]);
    return Object.fromEntries(entries) as Record<SiteStatus, number>;
}

function utilisationFor(countryCode: string): CountryUtilisation {
    const sites = MOCK_SITES.filter(s => s.countryCode === countryCode);
    const capacities = towersOf(sites);
    const totalSlots = capacities.reduce((sum, c) => sum + c.maxTenants, 0);
    const occupiedSlots = capacities.reduce((sum, c) => sum + c.activeTenantCount, 0);
    return { countryCode, siteCount: sites.length, occupiedSlots, totalSlots, utilisationPercent: percentOf(occupiedSlots, totalSlots) };
}

function buildUtilisationByCountry(): CountryUtilisation[] {
    const codes = [...new Set(MOCK_SITES.map(s => s.countryCode))];
    return codes.map(utilisationFor).sort((a, b) => b.utilisationPercent - a.utilisationPercent);
}

function buildRentByCurrency(): CurrencyAmount[] {
    const totals = new Map<string, number>();
    activeTenancies().forEach(t => totals.set(t.currency, (totals.get(t.currency) ?? 0) + t.monthlyRent));
    return [...totals].map(([currency, amount]) => ({ currency, amount }));
}

function buildOperatorShares(): OperatorShare[] {
    const active = activeTenancies();
    return MOCK_OPERATORS
        .map(o => ({ operatorId: o.id, operatorName: o.name, activeTenancies: active.filter(t => t.operatorId === o.id).length }))
        .sort((a, b) => b.activeTenancies - a.activeTenancies);
}

function toExpiring(tenancy: MockTenancy, today: Date): ExpiringTenancy {
    const site = siteOfTower(tenancy.towerId);
    const daysRemaining = Math.ceil((new Date(tenancy.endDate!).getTime() - today.getTime()) / MS_PER_DAY);
    const operatorName = MOCK_OPERATORS.find(o => o.id === tenancy.operatorId)?.name ?? '';
    return { tenancyId: tenancy.id, siteId: site.id, siteCode: site.siteCode, operatorName, endDate: tenancy.endDate!, daysRemaining };
}

function buildExpiringTenancies(): ExpiringTenancy[] {
    const today = new Date();
    return activeTenancies()
        .filter(t => t.endDate !== null)
        .map(t => toExpiring(t, today))
        .filter(e => e.daysRemaining <= RENEWAL_WINDOW_DAYS)
        .sort((a, b) => a.daysRemaining - b.daysRemaining);
}

function buildSellableTowers(): SellableTower[] {
    return MOCK_SITES
        .filter(s => s.status === 'Active')
        .map(s => ({ site: s, capacity: towersOf([s])[0] }))
        .filter(({ capacity }) => capacity.hasCapacity)
        .map(({ site, capacity }) => ({ siteId: site.id, siteCode: site.siteCode, siteName: site.name, countryCode: site.countryCode, availableSlots: capacity.availableSlots }))
        .sort((a, b) => b.availableSlots - a.availableSlots);
}

function buildDashboard(): PortfolioDashboard {
    const expiringTenancies = buildExpiringTenancies();
    return {
        kpis: buildKpis(),
        sitesByStatus: buildSitesByStatus(),
        utilisationByCountry: buildUtilisationByCountry(),
        monthlyRentByCurrency: buildRentByCurrency(),
        operatorShares: buildOperatorShares(),
        expiringTenancies,
        sellableTowers: buildSellableTowers(),
        alerts: buildAlerts(expiringTenancies),
    };
}

export const mockDashboardApi: DashboardApi = {
    get: () => delay(buildDashboard),
};
