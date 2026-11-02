import { API_ENDPOINTS } from '@/constants/api';
import type { TenantFilter, TenantRecord } from '@/types';
import type { TenantsApi } from './contracts';
import { httpClient } from './httpClient';
import { toQueryString } from './queryString';

export const tenantsApi: TenantsApi = {
    list: (filter: TenantFilter = {}) =>
        httpClient.get<TenantRecord[]>(`${API_ENDPOINTS.tenancies}${toQueryString(filter)}`),
};
