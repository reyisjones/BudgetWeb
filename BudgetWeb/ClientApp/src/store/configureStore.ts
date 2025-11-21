import { combineReducers, configureStore as createReduxStore } from '@reduxjs/toolkit';
import { ApplicationState, reducers } from './';

export default function configureStore(initialState?: ApplicationState) {
    const rootReducer = combineReducers(reducers);

    return createReduxStore({
        reducer: rootReducer,
        middleware: (getDefaultMiddleware) => getDefaultMiddleware(),
        preloadedState: initialState,
        devTools: process.env.NODE_ENV !== 'production',
    });
}

