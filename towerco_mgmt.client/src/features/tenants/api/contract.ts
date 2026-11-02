import type { TenantFilter, TenantRecord } from '@/features/tenants/types';

export interface TenantsApi {
    list: (filter?: TenantFilter) => Promise<TenantRecord[]>;
}
