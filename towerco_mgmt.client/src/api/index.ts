import { USE_MOCK_API } from '@/constants/api';
import { mockDashboardApi } from './mock/mockDashboard';
import { mockOperatorsApi, mockSitesApi, mockTowersApi } from './mock/mockApi';
import { mockEquipmentApi } from './mock/mockEquipment';
import { mockTenantsApi } from './mock/mockTenants';
import { dashboardApi as httpDashboardApi } from './dashboardApi';
import { equipmentApi as httpEquipmentApi } from './equipmentApi';
import { tenantsApi as httpTenantsApi } from './tenantsApi';
import { operatorsApi as httpOperatorsApi } from './operatorsApi';
import { sitesApi as httpSitesApi } from './sitesApi';
import { towersApi as httpTowersApi } from './towersApi';
import { mockWorkOrdersApi } from './mock/mockWorkOrders';
import { workOrdersApi as httpWorkOrdersApi } from './workOrdersApi';

export { ApiError } from './ApiError';
export { tenanciesApi } from './tenanciesApi';
/** Health has no mock: telemetry only exists in the backend. */
export { healthApi } from './healthApi';

export const sitesApi = USE_MOCK_API ? mockSitesApi : httpSitesApi;
export const operatorsApi = USE_MOCK_API ? mockOperatorsApi : httpOperatorsApi;
export const towersApi = USE_MOCK_API ? mockTowersApi : httpTowersApi;
export const dashboardApi = USE_MOCK_API ? mockDashboardApi : httpDashboardApi;
export const workOrdersApi = USE_MOCK_API ? mockWorkOrdersApi : httpWorkOrdersApi;
export const tenantsApi = USE_MOCK_API ? mockTenantsApi : httpTenantsApi;
export const equipmentApi = USE_MOCK_API ? mockEquipmentApi : httpEquipmentApi;
