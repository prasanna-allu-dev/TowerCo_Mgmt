import { API_ENDPOINTS } from '@/constants/api';
import type { CreateSiteRequest, SiteDetail, SiteFilter, SiteSummary } from '@/types';
import type { SitesApi } from './contracts';
import { httpClient } from './httpClient';

function toQueryString(filter: SiteFilter): string {
    const params = new URLSearchParams();
    Object.entries(filter).forEach(([key, value]) => value && params.append(key, value));
    const query = params.toString();
    return query ? `?${query}` : '';
}

export const sitesApi: SitesApi = {
    list: (filter: SiteFilter = {}) =>
        httpClient.get<SiteSummary[]>(`${API_ENDPOINTS.sites}${toQueryString(filter)}`),
    get: (id: number) => httpClient.get<SiteDetail>(API_ENDPOINTS.site(id)),
    create: (request: CreateSiteRequest) => httpClient.post<SiteDetail>(API_ENDPOINTS.sites, request),
};
