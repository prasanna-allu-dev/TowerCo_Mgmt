import { API_ENDPOINTS } from '@/shared/constants/api';
import type { EquipmentFilter, EquipmentItem, EquipmentTypeSummary } from '@/features/equipment/types';
import type { EquipmentApi } from '@/features/equipment/api';
import { httpClient } from '@/shared/api/httpClient';
import { toQueryString } from '@/shared/api/queryString';

export const equipmentApi: EquipmentApi = {
    list: (filter: EquipmentFilter = {}) =>
        httpClient.get<EquipmentItem[]>(`${API_ENDPOINTS.equipment}${toQueryString(filter)}`),
    types: () => httpClient.get<EquipmentTypeSummary[]>(API_ENDPOINTS.equipmentTypes),
};
