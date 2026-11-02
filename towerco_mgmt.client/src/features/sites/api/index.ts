import { USE_MOCK_API } from '@/shared/constants/api';
import { mockSitesApi } from '@/mocks/mockApi';
import { sitesApi as httpSitesApi } from './sitesApi';

export type * from './contract';
export const sitesApi = USE_MOCK_API ? mockSitesApi : httpSitesApi;
