import { API_ENDPOINTS } from '@/constants/api';
import type { HealthFilter, PortfolioHealth, SiteHealth } from '@/types';
import type { HealthApi } from './contracts';
import { httpClient } from './httpClient';
import { toQueryString } from './queryString';

export const healthApi: HealthApi = {
    portfolio: (filter: HealthFilter = {}) =>
        httpClient.get<PortfolioHealth>(`${API_ENDPOINTS.health}${toQueryString(filter)}`),
    site: (siteId: number) => httpClient.get<SiteHealth>(API_ENDPOINTS.siteHealth(siteId)),
};
