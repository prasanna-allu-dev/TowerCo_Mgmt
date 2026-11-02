import type { WorkOrderFilter, WorkOrder, CreateWorkOrderRequest, UpdateWorkOrderStatusRequest } from '@/features/workOrders/types';

export interface WorkOrdersApi {
    list: (filter?: WorkOrderFilter) => Promise<WorkOrder[]>;
    get: (id: number) => Promise<WorkOrder>;
    create: (request: CreateWorkOrderRequest) => Promise<WorkOrder>;
    updateStatus: (id: number, request: UpdateWorkOrderStatusRequest) => Promise<WorkOrder>;
}
