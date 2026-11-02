import type { CapacitySummary, SiteStatus } from './site';

export interface TowerConfiguration {
    towerId: number;
    siteId: number;
    siteCode: string;
    siteName: string;
    countryCode: string;
    siteStatus: SiteStatus;
    towerType: string;
    heightMetres: number;
    capacity: CapacitySummary;
}
