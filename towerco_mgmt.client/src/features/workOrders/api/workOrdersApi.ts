import { API_ENDPOINTS } from '@/shared/constants/api';
import type { CreateWorkOrderRequest, UpdateWorkOrderStatusRequest, WorkOrder, WorkOrderFilter } from '@/features/workOrders/types';
import type { WorkOrdersApi } from '@/features/workOrders/api';
import { httpClient } from '@/shared/api/httpClient';
import { toQueryString } from '@/shared/api/queryString';

export const workOrdersApi: WorkOrdersApi = {
    list: (filter: WorkOrderFilter = {}) =>
        httpClient.get<WorkOrder[]>(`${API_ENDPOINTS.workOrders}${toQueryString(filter)}`),
    get: (id: number) => httpClient.get<WorkOrder>(API_ENDPOINTS.workOrder(id)),
    create: (request: CreateWorkOrderRequest) => httpClient.post<WorkOrder>(API_ENDPOINTS.workOrders, request),
    updateStatus: (id: number, request: UpdateWorkOrderStatusRequest) =>
        httpClient.patch<WorkOrder>(API_ENDPOINTS.workOrderStatus(id), request),
};
