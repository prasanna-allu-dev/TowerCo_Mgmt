import type { Operator } from '@/features/operators/types';

export interface OperatorsApi {
    list: () => Promise<Operator[]>;
}
