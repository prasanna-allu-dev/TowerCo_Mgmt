import type { SiteFilter, SiteSummary, SiteDetail, CreateSiteRequest } from '@/features/sites/types';

export interface SitesApi {
    list: (filter?: SiteFilter) => Promise<SiteSummary[]>;
    get: (id: number) => Promise<SiteDetail>;
    create: (request: CreateSiteRequest) => Promise<SiteDetail>;
}
