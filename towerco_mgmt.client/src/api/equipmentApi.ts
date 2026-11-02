import { API_ENDPOINTS } from '@/constants/api';
import type { EquipmentFilter, EquipmentItem, EquipmentTypeSummary } from '@/types';
import type { EquipmentApi } from './contracts';
import { httpClient } from './httpClient';
import { toQueryString } from './queryString';

export const equipmentApi: EquipmentApi = {
    list: (filter: EquipmentFilter = {}) =>
        httpClient.get<EquipmentItem[]>(`${API_ENDPOINTS.equipment}${toQueryString(filter)}`),
    types: () => httpClient.get<EquipmentTypeSummary[]>(API_ENDPOINTS.equipmentTypes),
};
