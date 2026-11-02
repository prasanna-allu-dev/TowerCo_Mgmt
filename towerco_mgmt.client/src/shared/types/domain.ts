import type { SITE_STATUSES } from '@/shared/constants/domain';

export type SiteStatus = (typeof SITE_STATUSES)[number];

export type Severity = 'ok' | 'warning' | 'critical' | 'neutral';

export interface CapacitySummary {
    maxTenants: number;
    activeTenantCount: number;
    availableSlots: number;
    utilisationPercent: number;
    hasCapacity: boolean;
}
