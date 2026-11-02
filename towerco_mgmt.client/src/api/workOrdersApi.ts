import { API_ENDPOINTS } from '@/constants/api';
import type { CreateWorkOrderRequest, UpdateWorkOrderStatusRequest, WorkOrder, WorkOrderFilter } from '@/types';
import type { WorkOrdersApi } from './contracts';
import { httpClient } from './httpClient';
import { toQueryString } from './queryString';

export const workOrdersApi: WorkOrdersApi = {
    list: (filter: WorkOrderFilter = {}) =>
        httpClient.get<WorkOrder[]>(`${API_ENDPOINTS.workOrders}${toQueryString(filter)}`),
    get: (id: number) => httpClient.get<WorkOrder>(API_ENDPOINTS.workOrder(id)),
    create: (request: CreateWorkOrderRequest) => httpClient.post<WorkOrder>(API_ENDPOINTS.workOrders, request),
    updateStatus: (id: number, request: UpdateWorkOrderStatusRequest) =>
        httpClient.patch<WorkOrder>(API_ENDPOINTS.workOrderStatus(id), request),
};
