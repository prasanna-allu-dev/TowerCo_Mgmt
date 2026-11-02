import type { EQUIPMENT_CATEGORIES, EQUIPMENT_STATUSES } from '@/shared/constants/domain';

export type EquipmentCategory = (typeof EQUIPMENT_CATEGORIES)[number];
export type EquipmentStatus = (typeof EQUIPMENT_STATUSES)[number];

export interface EquipmentType {
    id: number;
    name: string;
    category: EquipmentCategory;
}

export interface EquipmentTypeSummary extends EquipmentType {
    itemCount: number;
}

export interface EquipmentItem {
    id: number;
    towerId: number;
    siteId: number;
    siteCode: string;
    siteName: string;
    countryCode: string;
    typeId: number;
    typeName: string;
    category: EquipmentCategory;
    tenancyId: number | null;
    ownerName: string | null;
    serialNumber: string;
    installedOn: string;
    mountHeightMetres: number | null;
    status: EquipmentStatus;
}

export interface EquipmentFilter {
    search?: string;
    category?: string;
    owner?: string;
    status?: string;
    country?: string;
    siteId?: number;
}
