import { API_ENDPOINTS } from '@/constants/api';
import type { PortfolioDashboard } from '@/types';
import type { DashboardApi } from './contracts';
import { httpClient } from './httpClient';

export const dashboardApi: DashboardApi = {
    get: () => httpClient.get<PortfolioDashboard>(API_ENDPOINTS.dashboard),
};
