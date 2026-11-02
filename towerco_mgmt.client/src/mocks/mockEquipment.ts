import type { EquipmentFilter, EquipmentItem, EquipmentStatus, EquipmentType, EquipmentTypeSummary } from '@/features/equipment/types';
import type { EquipmentApi } from '@/features/equipment/api';
import { delay, isActive } from './mockApi';
import { MOCK_OPERATORS, MOCK_SITES, MOCK_TENANCIES, MOCK_TOWERS } from './mockData';
import type { MockTower } from './mockData';

// Fictional demo data. Mirrors the equipment seed: two tenant-owned items per tenancy, no TowerCo-owned kit.
export const MOCK_EQUIPMENT_TYPES: EquipmentType[] = [
    { id: 1, name: '5G Active Antenna (AAU)', category: 'Radio' },
    { id: 2, name: '4G Remote Radio Unit', category: 'Radio' },
    { id: 3, name: 'Microwave Dish', category: 'Transmission' },
    { id: 4, name: 'Diesel Generator', category: 'Power' },
    { id: 5, name: 'Battery Bank', category: 'Power' },
    { id: 6, name: 'Solar Array', category: 'Power' },
    { id: 7, name: 'Air Conditioner', category: 'Cooling' },
    { id: 8, name: 'Security Camera', category: 'Security' },
];

const TYPE_5G = 1;
const TYPE_4G = 2;
const TYPE_DISH = 3;
const MOUNT_STEP_METRES = 4;
const DISH_DROP_METRES = 2;
const SERIAL_PREFIX = 'EQ';
const SERIAL_DIGITS = 4;

interface Draft {
    towerId: number;
    typeId: number;
    tenancyId: number;
    installedOn: string;
    mountHeightMetres: number | null;
    status: EquipmentStatus;
}

interface MockEquipment extends Draft {
    id: number;
    serialNumber: string;
}

function tenantDrafts(tower: MockTower): Draft[] {
    return MOCK_TENANCIES.filter(t => t.towerId === tower.id).flatMap((tenancy, index) => {
        const mount = tower.heightMetres - MOUNT_STEP_METRES * (index + 1);
        const status: EquipmentStatus = isActive(tenancy.endDate) ? 'Installed' : 'Removed';
        const base = { towerId: tower.id, tenancyId: tenancy.id, installedOn: tenancy.startDate, status };
        const radio = tenancy.id % 2 ? TYPE_5G : TYPE_4G;
        return [
            { ...base, typeId: radio, mountHeightMetres: mount },
            { ...base, typeId: TYPE_DISH, mountHeightMetres: mount - DISH_DROP_METRES },
        ];
    });
}

const serialOf = (id: number) => `${SERIAL_PREFIX}-${String(id).padStart(SERIAL_DIGITS, '0')}`;

export const MOCK_EQUIPMENT: MockEquipment[] = MOCK_TOWERS
    .flatMap(tenantDrafts)
    .map((draft, index) => ({ ...draft, id: index + 1, serialNumber: serialOf(index + 1) }));

function toItem(row: MockEquipment): EquipmentItem {
    const tower = MOCK_TOWERS.find(t => t.id === row.towerId)!;
    const site = MOCK_SITES.find(s => s.id === tower.siteId)!;
    const type = MOCK_EQUIPMENT_TYPES.find(t => t.id === row.typeId)!;
    const tenancy = MOCK_TENANCIES.find(t => t.id === row.tenancyId)!;
    const ownerName = MOCK_OPERATORS.find(o => o.id === tenancy.operatorId)!.name;
    return {
        ...row, siteId: site.id, siteCode: site.siteCode, siteName: site.name, countryCode: site.countryCode,
        typeName: type.name, category: type.category, ownerName,
    };
}

function textMatches(item: EquipmentItem, search?: string): boolean {
    const text = search?.toLowerCase();
    const fields = [item.serialNumber, item.typeName, item.siteCode, item.siteName, item.ownerName];
    return !text || fields.some(v => v.toLowerCase().includes(text));
}

function matches(item: EquipmentItem, filter: EquipmentFilter): boolean {
    return textMatches(item, filter.search)
        && (!filter.category || item.category === filter.category)
        && (!filter.status || item.status === filter.status)
        && (!filter.country || item.countryCode === filter.country)
        && (!filter.siteId || item.siteId === filter.siteId);
}

function summariseTypes(): EquipmentTypeSummary[] {
    return MOCK_EQUIPMENT_TYPES.map(type => ({
        ...type, itemCount: MOCK_EQUIPMENT.filter(e => e.typeId === type.id && e.status !== 'Removed').length,
    }));
}

export const mockEquipmentApi: EquipmentApi = {
    list: (filter = {}) => delay(() => MOCK_EQUIPMENT.map(toItem).filter(item => matches(item, filter))),
    types: () => delay(summariseTypes),
};
