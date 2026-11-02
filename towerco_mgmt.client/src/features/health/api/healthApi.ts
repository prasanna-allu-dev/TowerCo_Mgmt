import { API_ENDPOINTS } from '@/shared/constants/api';
import type { HealthFilter, PortfolioHealth, SiteHealth } from '@/features/health/types';
import type { HealthApi } from '@/features/health/api';
import { httpClient } from '@/shared/api/httpClient';
import { toQueryString } from '@/shared/api/queryString';

export const healthApi: HealthApi = {
    portfolio: (filter: HealthFilter = {}) =>
        httpClient.get<PortfolioHealth>(`${API_ENDPOINTS.health}${toQueryString(filter)}`),
    site: (siteId: number) => httpClient.get<SiteHealth>(API_ENDPOINTS.siteHealth(siteId)),
};
