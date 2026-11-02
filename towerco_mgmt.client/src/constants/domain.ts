export const SITE_STATUSES = ['Planned', 'UnderConstruction', 'Active', 'Decommissioned'] as const;

export const COUNTRIES = [
    { code: 'IN', name: 'India' },
    { code: 'ZA', name: 'South Africa' },
    { code: 'KE', name: 'Kenya' },
    { code: 'NG', name: 'Nigeria' },
] as const;

export const EQUIPMENT_CATEGORIES = ['Radio', 'Transmission', 'Power', 'Cooling', 'Security'] as const;
export const EQUIPMENT_STATUSES = ['Planned', 'Installed', 'Removed'] as const;

export const OWNER_TENANT = 'tenant';
export const OWNER_SHARED = 'shared';
export const SHARED_OWNER_LABEL = 'TowerCo shared';
export const OWNER_FILTERS = [
    { value: OWNER_TENANT, label: 'Tenant-owned' },
    { value: OWNER_SHARED, label: SHARED_OWNER_LABEL },
] as const;

export const TENANCY_ACTIVE = 'active';
export const TENANCY_ENDED = 'ended';
export const TENANCY_STATES = [
    { value: TENANCY_ACTIVE, label: 'Active' },
    { value: TENANCY_ENDED, label: 'Ended' },
] as const;

export const UTILISATION_WARNING_PERCENT = 75;

export const DISPLAY_LOCALE = 'en-IN';
export const OPEN_ENDED_LABEL = 'Open ended';

export const RENEWAL_WINDOW_DAYS = 90;
export const MS_PER_DAY = 86_400_000;
export const RENEWAL_CRITICAL_DAYS = 30;

export const SEVERITY_LABELS = { ok: 'Healthy', warning: 'Warning', critical: 'Critical', neutral: 'Info' } as const;

export const TOWER_TYPES = ['Monopole', 'Lattice', 'Guyed', 'Rooftop'] as const;
export const DEFAULT_NEW_SITE_STATUS = 'Planned';
export const SITE_CODE_PATTERN = /^[A-Z]{2}-[A-Z]{3}-\d{4}$/;
export const SITE_CODE_EXAMPLE = 'IN-PUN-0005';
export const TOWER_LIMITS = {
    minHeightMetres: 5,
    maxHeightMetres: 120,
    minTenants: 1,
    maxTenants: 6,
} as const;
export const LATITUDE_LIMIT = 90;
export const LONGITUDE_LIMIT = 180;
