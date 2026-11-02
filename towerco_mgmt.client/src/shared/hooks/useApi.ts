import { useCallback, useEffect, useState } from 'react';
import { API_ERROR_FALLBACK } from '@/shared/constants/api';

interface ApiState<T> {
    data: T | undefined;
    error: string | undefined;
    loading: boolean;
    reload: () => void;
}

export function useApi<T>(fetcher: () => Promise<T>, deps: unknown[] = []): ApiState<T> {
    const [data, setData] = useState<T>();
    const [error, setError] = useState<string>();
    const [loading, setLoading] = useState(true);

    const load = useCallback(() => {
        setLoading(true);
        setError(undefined);
        fetcher()
            .then(setData)
            .catch((e: unknown) => setError(e instanceof Error ? e.message : API_ERROR_FALLBACK))
            .finally(() => setLoading(false));
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, deps);

    useEffect(load, [load]);
    return { data, error, loading, reload: load };
}
