export const ROUTES = {
    sites: '/sites',
    siteDetail: '/sites/:siteId',
    operators: '/operators',
    tenants: '/tenants',
    equipment: '/equipment',
    configuration: '/configuration',
    newTowerSite: '/configuration/new',
    dashboard: '/dashboard',
    workOrders: '/work-orders',
    workOrderDetail: '/work-orders/:workOrderId',
    health: '/health',
} as const;

export const sitePath = (id: number) => `${ROUTES.sites}/${id}`;
export const workOrderPath = (id: number) => `${ROUTES.workOrders}/${id}`;
