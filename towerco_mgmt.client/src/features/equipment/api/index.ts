import { USE_MOCK_API } from '@/shared/constants/api';
import { mockEquipmentApi } from '@/mocks/mockEquipment';
import { equipmentApi as httpEquipmentApi } from './equipmentApi';

export type * from './contract';
export const equipmentApi = USE_MOCK_API ? mockEquipmentApi : httpEquipmentApi;
