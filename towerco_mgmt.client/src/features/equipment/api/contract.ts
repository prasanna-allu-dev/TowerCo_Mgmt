import type { EquipmentFilter, EquipmentItem, EquipmentTypeSummary } from '@/features/equipment/types';

export interface EquipmentApi {
    list: (filter?: EquipmentFilter) => Promise<EquipmentItem[]>;
    types: () => Promise<EquipmentTypeSummary[]>;
}
