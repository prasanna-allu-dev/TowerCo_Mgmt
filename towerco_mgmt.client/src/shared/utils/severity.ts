import { RENEWAL_CRITICAL_DAYS, RENEWAL_WINDOW_DAYS, UTILISATION_WARNING_PERCENT } from '@/shared/constants/domain';
import type { Severity } from '@/shared/types/domain';
import type { SiteStatus } from '@/shared/types/domain';

const FULL_PERCENT = 100;

const STATUS_SEVERITY: Record<SiteStatus, Severity> = {
    Active: 'ok', UnderConstruction: 'warning', Planned: 'neutral', Decommissioned: 'critical',
};

export function utilisationSeverity(percent: number): Severity {
    if (percent >= FULL_PERCENT) return 'critical';
    return percent >= UTILISATION_WARNING_PERCENT ? 'warning' : 'ok';
}

export function renewalSeverity(daysRemaining: number): Severity {
    if (daysRemaining <= RENEWAL_CRITICAL_DAYS) return 'critical';
    return daysRemaining <= RENEWAL_WINDOW_DAYS ? 'warning' : 'ok';
}

export const countSeverity = (count: number, whenPresent: Severity): Severity => (count > 0 ? whenPresent : 'ok');

export const statusSeverity = (status: SiteStatus): Severity => STATUS_SEVERITY[status];

const SEVERITY_RANK: Record<Severity, number> = { critical: 0, warning: 1, neutral: 2, ok: 3 };

export const bySeverity = (a: { severity: Severity }, b: { severity: Severity }) => SEVERITY_RANK[a.severity] - SEVERITY_RANK[b.severity];
