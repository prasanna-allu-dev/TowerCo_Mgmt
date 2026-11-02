import type { Operator, SiteStatus } from '@/types';

// Fictional demo data. Mirrors the backend seed shape until the read API exists.
export interface MockSite {
    id: number;
    siteCode: string;
    name: string;
    countryCode: string;
    city: string;
    latitude: number;
    longitude: number;
    status: SiteStatus;
}

export interface MockTower {
    id: number;
    siteId: number;
    towerType: string;
    heightMetres: number;
    maxTenants: number;
}

export interface MockTenancy {
    id: number;
    towerId: number;
    operatorId: number;
    startDate: string;
    endDate: string | null;
    monthlyRent: number;
    currency: string;
}

export const MOCK_OPERATORS: Operator[] = [
    { id: 1, name: 'Skyline Mobile', countryCode: 'IN' },
    { id: 2, name: 'Savanna Telecom', countryCode: 'KE' },
    { id: 3, name: 'Baobab Wireless', countryCode: 'NG' },
    { id: 4, name: 'Cape Connect', countryCode: 'ZA' },
    { id: 5, name: 'Monsoon Networks', countryCode: 'IN' },
];

export const MOCK_SITES: MockSite[] = [
    { id: 1, siteCode: 'IN-MUM-0001', name: 'Bandra Ridge', countryCode: 'IN', city: 'Mumbai', latitude: 19.06, longitude: 72.83, status: 'Active' },
    { id: 2, siteCode: 'IN-BLR-0001', name: 'Whitefield East', countryCode: 'IN', city: 'Bengaluru', latitude: 12.97, longitude: 77.75, status: 'Active' },
    { id: 3, siteCode: 'IN-DEL-0001', name: 'Dwarka Sector 9', countryCode: 'IN', city: 'Delhi', latitude: 28.58, longitude: 77.05, status: 'UnderConstruction' },
    { id: 4, siteCode: 'ZA-JNB-0001', name: 'Sandton Heights', countryCode: 'ZA', city: 'Johannesburg', latitude: -26.11, longitude: 28.05, status: 'Active' },
    { id: 5, siteCode: 'ZA-CPT-0001', name: 'Table View', countryCode: 'ZA', city: 'Cape Town', latitude: -33.82, longitude: 18.49, status: 'Planned' },
    { id: 6, siteCode: 'ZA-DUR-0001', name: 'Umhlanga Point', countryCode: 'ZA', city: 'Durban', latitude: -29.73, longitude: 31.08, status: 'Decommissioned' },
    { id: 7, siteCode: 'KE-NBO-0001', name: 'Westlands Hill', countryCode: 'KE', city: 'Nairobi', latitude: -1.26, longitude: 36.8, status: 'Active' },
    { id: 8, siteCode: 'KE-MBA-0001', name: 'Nyali Coast', countryCode: 'KE', city: 'Mombasa', latitude: -4.04, longitude: 39.71, status: 'Active' },
    { id: 9, siteCode: 'KE-KSM-0001', name: 'Lakeside Kisumu', countryCode: 'KE', city: 'Kisumu', latitude: -0.09, longitude: 34.77, status: 'Planned' },
    { id: 10, siteCode: 'NG-LOS-0001', name: 'Lekki Phase 1', countryCode: 'NG', city: 'Lagos', latitude: 6.45, longitude: 3.47, status: 'Active' },
    { id: 11, siteCode: 'NG-ABV-0001', name: 'Maitama Crest', countryCode: 'NG', city: 'Abuja', latitude: 9.08, longitude: 7.49, status: 'Active' },
    { id: 12, siteCode: 'NG-PHC-0001', name: 'Trans Amadi', countryCode: 'NG', city: 'Port Harcourt', latitude: 4.81, longitude: 7.04, status: 'UnderConstruction' },
];

const TOWER_TYPES = ['Monopole', 'Lattice', 'Guyed', 'Rooftop'];
const TOWER_SLOTS = [3, 4, 2, 5];
const TOWER_ID_OFFSET = 100;
const BASE_HEIGHT_METRES = 30;
const HEIGHT_STEP_METRES = 10;

export const MOCK_TOWERS: MockTower[] = MOCK_SITES.map((site, index) => ({
    id: TOWER_ID_OFFSET + site.id,
    siteId: site.id,
    towerType: TOWER_TYPES[index % TOWER_TYPES.length],
    heightMetres: BASE_HEIGHT_METRES + (index % TOWER_TYPES.length) * HEIGHT_STEP_METRES,
    maxTenants: TOWER_SLOTS[index % TOWER_SLOTS.length],
}));

export const MOCK_TENANCIES: MockTenancy[] = [
    { id: 1, towerId: 101, operatorId: 1, startDate: '2024-01-01', endDate: null, monthlyRent: 85000, currency: 'INR' },
    { id: 2, towerId: 101, operatorId: 5, startDate: '2024-06-01', endDate: null, monthlyRent: 82000, currency: 'INR' },
    { id: 3, towerId: 101, operatorId: 3, startDate: '2025-02-01', endDate: null, monthlyRent: 80000, currency: 'INR' },
    { id: 4, towerId: 102, operatorId: 1, startDate: '2024-03-01', endDate: null, monthlyRent: 78000, currency: 'INR' },
    { id: 5, towerId: 104, operatorId: 4, startDate: '2023-09-01', endDate: null, monthlyRent: 21000, currency: 'ZAR' },
    { id: 6, towerId: 104, operatorId: 2, startDate: '2023-11-01', endDate: '2025-10-31', monthlyRent: 20000, currency: 'ZAR' },
    { id: 7, towerId: 107, operatorId: 2, startDate: '2024-05-01', endDate: null, monthlyRent: 145000, currency: 'KES' },
    { id: 8, towerId: 107, operatorId: 3, startDate: '2024-08-01', endDate: null, monthlyRent: 140000, currency: 'KES' },
    { id: 9, towerId: 107, operatorId: 4, startDate: '2025-01-01', endDate: null, monthlyRent: 138000, currency: 'KES' },
    { id: 10, towerId: 110, operatorId: 3, startDate: '2024-02-01', endDate: null, monthlyRent: 950000, currency: 'NGN' },
    { id: 11, towerId: 111, operatorId: 3, startDate: '2024-04-01', endDate: null, monthlyRent: 900000, currency: 'NGN' },
    { id: 12, towerId: 111, operatorId: 2, startDate: '2025-03-01', endDate: null, monthlyRent: 880000, currency: 'NGN' },
    { id: 13, towerId: 111, operatorId: 1, startDate: '2025-05-01', endDate: null, monthlyRent: 870000, currency: 'NGN' },
    { id: 14, towerId: 111, operatorId: 4, startDate: '2025-07-01', endDate: null, monthlyRent: 860000, currency: 'NGN' },
    { id: 15, towerId: 102, operatorId: 2, startDate: '2025-01-01', endDate: '2026-12-15', monthlyRent: 76000, currency: 'INR' },
    { id: 16, towerId: 108, operatorId: 4, startDate: '2025-02-01', endDate: '2027-01-20', monthlyRent: 132000, currency: 'KES' },
];
