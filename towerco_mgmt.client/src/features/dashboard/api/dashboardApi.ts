import { API_ENDPOINTS } from '@/shared/constants/api';
import type { PortfolioDashboard } from '@/features/dashboard/types';
import type { DashboardApi } from '@/features/dashboard/api';
import { httpClient } from '@/shared/api/httpClient';

export const dashboardApi: DashboardApi = {
    get: () => httpClient.get<PortfolioDashboard>(API_ENDPOINTS.dashboard),
};
