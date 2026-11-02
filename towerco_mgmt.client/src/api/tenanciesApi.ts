import { API_ENDPOINTS } from '@/constants/api';
import type { CreateTenancyRequest, Tenancy } from '@/types';
import { httpClient } from './httpClient';

export const tenanciesApi = {
    create: (towerId: number, request: CreateTenancyRequest) =>
        httpClient.post<Tenancy>(API_ENDPOINTS.towerTenancies(towerId), request),
};
