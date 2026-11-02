import type {
    CreateSiteRequest, EquipmentFilter, EquipmentItem, EquipmentTypeSummary, Operator, PortfolioDashboard, SiteDetail, SiteFilter,
    SiteSummary, TenantFilter, TenantRecord, TowerConfiguration,
} from '@/types';
import type { CreateWorkOrderRequest, UpdateWorkOrderStatusRequest, WorkOrder, WorkOrderFilter } from '@/types';
import type { HealthFilter, PortfolioHealth, SiteHealth } from '@/types';

export interface TenantsApi {
    list: (filter?: TenantFilter) => Promise<TenantRecord[]>;
}

export interface EquipmentApi {
    list: (filter?: EquipmentFilter) => Promise<EquipmentItem[]>;
    types: () => Promise<EquipmentTypeSummary[]>;
}

export interface SitesApi {
    list: (filter?: SiteFilter) => Promise<SiteSummary[]>;
    get: (id: number) => Promise<SiteDetail>;
    create: (request: CreateSiteRequest) => Promise<SiteDetail>;
}

export interface OperatorsApi {
    list: () => Promise<Operator[]>;
}

export interface TowersApi {
    list: () => Promise<TowerConfiguration[]>;
}

export interface DashboardApi {
    get: () => Promise<PortfolioDashboard>;
}

export interface WorkOrdersApi {
    list: (filter?: WorkOrderFilter) => Promise<WorkOrder[]>;
    get: (id: number) => Promise<WorkOrder>;
    create: (request: CreateWorkOrderRequest) => Promise<WorkOrder>;
    updateStatus: (id: number, request: UpdateWorkOrderStatusRequest) => Promise<WorkOrder>;
}

export interface HealthApi {
    portfolio: (filter?: HealthFilter) => Promise<PortfolioHealth>;
    site: (siteId: number) => Promise<SiteHealth>;
}
