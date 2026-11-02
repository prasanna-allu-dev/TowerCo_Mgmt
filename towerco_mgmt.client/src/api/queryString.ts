export function toQueryString(filter: object): string {
    const params = new URLSearchParams();
    Object.entries(filter).forEach(([key, value]) => value && params.append(key, String(value)));
    const query = params.toString();
    return query ? `?${query}` : '';
}
