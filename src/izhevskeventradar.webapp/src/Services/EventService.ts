import type { IEvent } from "../Interfaces/Events/IEvent";
import { request } from "../Utils/api";

const BASE_URL = '/api/events';

export const fetchEventsByDate = (date: Date, signal?: AbortSignal): Promise<IEvent[]> => {

    // Преобразуем Date в строку YYYY-MM-DD
    // 'sv-SE' (Швеция) использует этот формат
    const dateString = date.toLocaleDateString('sv-SE');

    const params = new URLSearchParams({ date: dateString });
    const path = `${BASE_URL}?${params.toString()}`;

    return request<IEvent[]>(path, { method: 'GET', signal });
};

export const fetchEventsByRange = (startDate: Date, endDate: Date, signal?: AbortSignal): Promise<IEvent[]> => {

    const params = new URLSearchParams({
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString(),
    });
    const path = `${BASE_URL}?${params.toString()}`;

    return request<IEvent[]>(path, { method: 'GET', signal });
};