import { useState } from 'react';
import { toErrorMessage } from '@/shared/utils/errors';

/** Runs a write call and tracks its pending and error state, so forms do not repeat try/catch/finally. */
export function useAsyncAction<TArgs extends unknown[], TResult>(
    action: (...args: TArgs) => Promise<TResult>,
    onSuccess: (result: TResult) => void,
) {
    const [pending, setPending] = useState(false);
    const [error, setError] = useState<string>();

    const run = async (...args: TArgs) => {
        setPending(true);
        setError(undefined);
        try {
            onSuccess(await action(...args));
        } catch (e) {
            setError(toErrorMessage(e));
        } finally {
            setPending(false);
        }
    };

    return { run, pending, error };
}
