import { API_ENDPOINTS } from '@/shared/constants/api';
import type { CreateTenancyRequest, Tenancy } from '@/features/tenants/types';
import { httpClient } from '@/shared/api/httpClient';

export const tenanciesApi = {
    create: (towerId: number, request: CreateTenancyRequest) =>
        httpClient.post<Tenancy>(API_ENDPOINTS.towerTenancies(towerId), request),
};
