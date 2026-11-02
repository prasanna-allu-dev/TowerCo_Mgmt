import { USE_MOCK_API } from '@/shared/constants/api';
import { mockOperatorsApi } from '@/mocks/mockApi';
import { operatorsApi as httpOperatorsApi } from './operatorsApi';

export type * from './contract';
export const operatorsApi = USE_MOCK_API ? mockOperatorsApi : httpOperatorsApi;
