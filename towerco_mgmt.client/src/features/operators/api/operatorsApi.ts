import { API_ENDPOINTS } from '@/shared/constants/api';
import type { Operator } from '@/features/operators/types';
import type { OperatorsApi } from '@/features/operators/api';
import { httpClient } from '@/shared/api/httpClient';

export const operatorsApi: OperatorsApi = {
    list: () => httpClient.get<Operator[]>(API_ENDPOINTS.operators),
};
