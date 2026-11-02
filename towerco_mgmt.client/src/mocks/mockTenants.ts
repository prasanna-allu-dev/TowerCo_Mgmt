import { TENANCY_ACTIVE } from '@/shared/constants/domain';
import type { TenantFilter, TenantRecord } from '@/features/tenants/types';
import type { TenantsApi } from '@/features/tenants/api';
import { delay, isActive } from './mockApi';
import { MOCK_EQUIPMENT } from './mockEquipment';
import { MOCK_OPERATORS, MOCK_SITES, MOCK_TENANCIES, MOCK_TOWERS } from './mockData';
import type { MockTenancy } from './mockData';

function toRecord(tenancy: MockTenancy): TenantRecord {
    const tower = MOCK_TOWERS.find(t => t.id === tenancy.towerId)!;
    const site = MOCK_SITES.find(s => s.id === tower.siteId)!;
    const operator = MOCK_OPERATORS.find(o => o.id === tenancy.operatorId)!;
    const { id, operatorId, startDate, endDate, monthlyRent, currency } = tenancy;
    return {
        tenancyId: id, operatorId, operatorName: operator.name, siteId: site.id, siteCode: site.siteCode,
        siteName: site.name, countryCode: site.countryCode, towerId: tower.id, startDate, endDate, monthlyRent, currency,
        isActive: isActive(endDate),
        equipmentCount: MOCK_EQUIPMENT.filter(e => e.tenancyId === id && e.status !== 'Removed').length,
    };
}

function matches(record: TenantRecord, filter: TenantFilter): boolean {
    const text = filter.search?.toLowerCase();
    const textHit = !text || [record.operatorName, record.siteCode, record.siteName].some(v => v.toLowerCase().includes(text));
    const stateHit = !filter.state || (filter.state === TENANCY_ACTIVE) === record.isActive;
    return textHit && stateHit
        && (!filter.operatorId || String(record.operatorId) === filter.operatorId)
        && (!filter.country || record.countryCode === filter.country);
}

export const mockTenantsApi: TenantsApi = {
    list: (filter = {}) => delay(() => MOCK_TENANCIES.map(toRecord).filter(r => matches(r, filter))),
};
