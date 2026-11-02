import { USE_MOCK_API } from '@/shared/constants/api';
import { mockWorkOrdersApi } from '@/mocks/mockWorkOrders';
import { workOrdersApi as httpWorkOrdersApi } from './workOrdersApi';

export type * from './contract';
export const workOrdersApi = USE_MOCK_API ? mockWorkOrdersApi : httpWorkOrdersApi;
