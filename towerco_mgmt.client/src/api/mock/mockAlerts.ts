import type { CapacitySummary, DashboardAlert, ExpiringTenancy } from '@/types';
import { bySeverity, renewalSeverity } from '@/utils/severity';
import { capacityOf } from './mockApi';
import { MOCK_SITES, MOCK_TOWERS } from './mockData';
import type { MockSite } from './mockData';

interface SiteCapacity {
    site: MockSite;
    capacity: CapacitySummary;
}

function siteCapacities(): SiteCapacity[] {
    return MOCK_SITES.map(site => ({ site, capacity: capacityOf(MOCK_TOWERS.find(t => t.siteId === site.id)!) }));
}

function fullTowerAlert({ site, capacity }: SiteCapacity): DashboardAlert {
    return {
        id: `full-${site.id}`, severity: 'critical', siteId: site.id,
        title: `Tower full: ${site.siteCode}`,
        detail: `${capacity.activeTenantCount} of ${capacity.maxTenants} slots used. New tenant demand cannot be served.`,
    };
}

function emptyTowerAlert({ site }: SiteCapacity): DashboardAlert {
    return {
        id: `empty-${site.id}`, severity: 'warning', siteId: site.id,
        title: `No tenants: ${site.siteCode}`,
        detail: `${site.name} is active but earns no rent.`,
    };
}

function decommissionedAlert({ site, capacity }: SiteCapacity): DashboardAlert {
    return {
        id: `decom-${site.id}`, severity: 'critical', siteId: site.id,
        title: `Tenants on decommissioned site: ${site.siteCode}`,
        detail: `${capacity.activeTenantCount} active tenancies need migration.`,
    };
}

function renewalAlert(row: ExpiringTenancy): DashboardAlert {
    return {
        id: `renewal-${row.tenancyId}`, severity: renewalSeverity(row.daysRemaining), siteId: row.siteId,
        title: `Tenancy ending: ${row.operatorName} on ${row.siteCode}`,
        detail: `Ends ${row.endDate}, ${row.daysRemaining} days left.`,
    };
}

function capacityAlerts(rows: SiteCapacity[]): DashboardAlert[] {
    const active = rows.filter(r => r.site.status === 'Active');
    const decommissioned = rows.filter(r => r.site.status === 'Decommissioned' && r.capacity.activeTenantCount > 0);
    return [
        ...active.filter(r => !r.capacity.hasCapacity).map(fullTowerAlert),
        ...active.filter(r => r.capacity.activeTenantCount === 0).map(emptyTowerAlert),
        ...decommissioned.map(decommissionedAlert),
    ];
}

export function buildAlerts(expiring: ExpiringTenancy[]): DashboardAlert[] {
    return [...capacityAlerts(siteCapacities()), ...expiring.map(renewalAlert)].sort(bySeverity);
}
