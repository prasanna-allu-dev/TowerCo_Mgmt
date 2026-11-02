import { USE_MOCK_API } from '@/shared/constants/api';
import { mockDashboardApi } from '@/mocks/mockDashboard';
import { dashboardApi as httpDashboardApi } from './dashboardApi';

export type * from './contract';
export const dashboardApi = USE_MOCK_API ? mockDashboardApi : httpDashboardApi;
