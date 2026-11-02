import { API_ENDPOINTS } from '@/constants/api';
import type { TowerConfiguration } from '@/types';
import type { TowersApi } from './contracts';
import { httpClient } from './httpClient';

export const towersApi: TowersApi = {
    list: () => httpClient.get<TowerConfiguration[]>(API_ENDPOINTS.towers),
};
