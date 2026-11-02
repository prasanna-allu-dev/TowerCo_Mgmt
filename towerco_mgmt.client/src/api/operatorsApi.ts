import { API_ENDPOINTS } from '@/constants/api';
import type { Operator } from '@/types';
import type { OperatorsApi } from './contracts';
import { httpClient } from './httpClient';

export const operatorsApi: OperatorsApi = {
    list: () => httpClient.get<Operator[]>(API_ENDPOINTS.operators),
};
