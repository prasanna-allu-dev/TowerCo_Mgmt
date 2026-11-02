import type { PersonaId } from '@/app/personas';
import type { SiteStatus } from '@/features/sites/types';

/** Mirrors Site.ChangeStatus in the domain; the API is the authority and returns 409 on any other move. */
export const SITE_TRANSITIONS: Record<SiteStatus, readonly SiteStatus[]> = {
    Planned: ['UnderConstruction', 'Decommissioned'],
    UnderConstruction: ['Active', 'Decommissioned'],
    Active: ['Decommissioned'],
    Decommissioned: [],
};

export const DECOMMISSIONED: SiteStatus = 'Decommissioned';

export const SITE_STATUS_ACTION_LABELS: Record<SiteStatus, string> = {
    Planned: 'Mark planned', UnderConstruction: 'Start construction', Active: 'Activate', Decommissioned: 'Decommission',
};

// UI gating only. Real enforcement needs backend authorisation, which is out of scope for this demo.
export const SITE_MANAGERS: readonly PersonaId[] = ['operations', 'admin'];
export const LEASE_MANAGERS: readonly PersonaId[] = ['commercial', 'admin'];

export const TENANCY_ACTIONS = [
    { value: 'end', label: 'End on', input: 'date' },
    { value: 'renew', label: 'Renew to', input: 'date' },
    { value: 'rent', label: 'Review rent to', input: 'number' },
] as const;

export type TenancyAction = (typeof TENANCY_ACTIONS)[number]['value'];
