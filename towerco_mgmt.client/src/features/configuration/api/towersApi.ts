import { API_ENDPOINTS } from '@/shared/constants/api';
import type { TowerConfiguration } from '@/features/configuration/types';
import type { TowersApi } from '@/features/configuration/api';
import { httpClient } from '@/shared/api/httpClient';

export const towersApi: TowersApi = {
    list: () => httpClient.get<TowerConfiguration[]>(API_ENDPOINTS.towers),
};
