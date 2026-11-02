import type { TowerConfiguration } from '@/features/configuration/types';

export interface TowersApi {
    list: () => Promise<TowerConfiguration[]>;
}
