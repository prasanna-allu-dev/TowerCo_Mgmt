import type { CapacitySummary, SiteStatus } from '@/shared/types/domain';
import type { Tenancy } from '@/features/tenants/types/tenancy';

export type { CapacitySummary, SiteStatus } from '@/shared/types/domain';

export interface SiteSummary {
    id: number;
    siteCode: string;
    name: string;
    countryCode: string;
    city: string;
    status: SiteStatus;
    towerId: number;
    capacity: CapacitySummary;
}

export interface Tower {
    id: number;
    towerType: string;
    heightMetres: number;
    maxTenants: number;
}

export interface SiteDetail extends SiteSummary {
    latitude: number;
    longitude: number;
    tower: Tower;
    tenancies: Tenancy[];
}

/** UC1 thin slice: registers a site and its tower in one request (POST /api/sites). */
export interface CreateSiteRequest {
    siteCode: string;
    name: string;
    countryCode: string;
    city: string;
    latitude: number;
    longitude: number;
    towerType: string;
    heightMetres: number;
    maxTenants: number;
}

export interface SiteFilter {
    country?: string;
    status?: string;
    search?: string;
}
