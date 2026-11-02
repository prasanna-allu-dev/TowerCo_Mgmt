import { USE_MOCK_API } from '@/shared/constants/api';
import { mockTowersApi } from '@/mocks/mockApi';
import { towersApi as httpTowersApi } from './towersApi';

export type * from './contract';
export const towersApi = USE_MOCK_API ? mockTowersApi : httpTowersApi;
