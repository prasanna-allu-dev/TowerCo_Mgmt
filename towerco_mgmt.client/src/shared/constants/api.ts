import { sitePath } from '@/shared/constants/routes';
export const API_BASE_URL = '/api';

export const API_ENDPOINTS = {
    sites: '/sites',
    site: (id: number) => sitePath(id),
    operators: '/operators',
    towers: '/towers',
    dashboard: '/dashboard',
    tenancies: '/tenancies',
    equipment: '/equipment',
    equipmentTypes: '/equipment-types',
    towerTenancies: (towerId: number) => `/towers/${towerId}/tenancies`,
    workOrders: '/work-orders',
    workOrder: (id: number) => `/work-orders/${id}`,
    workOrderStatus: (id: number) => `/work-orders/${id}/status`,
    health: '/health/sites',
    siteHealth: (siteId: number) => `/health/sites/${siteId}`,
} as const;

export const API_ERROR_FALLBACK = 'Unable to reach the server. Please try again.';

export const USE_MOCK_API = import.meta.env.VITE_USE_MOCK_API === 'true';
export const MOCK_LATENCY_MS = Number(import.meta.env.VITE_MOCK_LATENCY_MS ?? 0);
export const MOCK_NOT_FOUND_STATUS = 404;
export const MOCK_CONFLICT_STATUS = 409;
export const MOCK_VALIDATION_STATUS = 400;
