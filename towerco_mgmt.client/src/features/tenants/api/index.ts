import { USE_MOCK_API } from '@/shared/constants/api';
import { mockTenantsApi } from '@/mocks/mockTenants';
import { tenantsApi as httpTenantsApi } from './tenantsApi';

export type * from './contract';
export { tenanciesApi } from './tenanciesApi';
export const tenantsApi = USE_MOCK_API ? mockTenantsApi : httpTenantsApi;
