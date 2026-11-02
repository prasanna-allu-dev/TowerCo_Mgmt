import { API_ENDPOINTS } from '@/shared/constants/api';
import type { TenantFilter, TenantRecord } from '@/features/tenants/types';
import type { TenantsApi } from '@/features/tenants/api';
import { httpClient } from '@/shared/api/httpClient';
import { toQueryString } from '@/shared/api/queryString';

export const tenantsApi: TenantsApi = {
    list: (filter: TenantFilter = {}) =>
        httpClient.get<TenantRecord[]>(`${API_ENDPOINTS.tenancies}${toQueryString(filter)}`),
};
