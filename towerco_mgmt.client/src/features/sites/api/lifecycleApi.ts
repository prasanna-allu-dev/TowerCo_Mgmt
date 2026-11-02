import { API_ENDPOINTS } from '@/shared/constants/api';
import { httpClient } from '@/shared/api/httpClient';
import type { CapacitySummary, SiteDetail, SiteStatus } from '@/features/sites/types';
import type { Tenancy } from '@/features/tenants/types';

/** Lifecycle commands. Backend only (no mock): the business rules live in the domain model. */
export const lifecycleApi = {
    changeStatus: (siteId: number, status: SiteStatus) =>
        httpClient.patch<SiteDetail>(API_ENDPOINTS.siteStatus(siteId), { status }),
    changeCapacity: (towerId: number, maxTenants: number) =>
        httpClient.patch<CapacitySummary>(API_ENDPOINTS.towerCapacity(towerId), { maxTenants }),
    endTenancy: (towerId: number, tenancyId: number, endDate: string) =>
        httpClient.post<Tenancy>(API_ENDPOINTS.tenancyEnd(towerId, tenancyId), { endDate }),
    renewTenancy: (towerId: number, tenancyId: number, endDate: string | null) =>
        httpClient.post<Tenancy>(API_ENDPOINTS.tenancyRenewal(towerId, tenancyId), { endDate }),
    reviewRent: (towerId: number, tenancyId: number, monthlyRent: number) =>
        httpClient.post<Tenancy>(API_ENDPOINTS.tenancyRentReview(towerId, tenancyId), { monthlyRent }),
};
