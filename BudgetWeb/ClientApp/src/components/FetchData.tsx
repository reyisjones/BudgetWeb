import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import { useParams } from 'react-router-dom';
import { Link } from 'react-router-dom';
import { ApplicationState } from '../store';
import * as WeatherForecastsStore from '../store/WeatherForecasts';

type WeatherForecastProps =
  WeatherForecastsStore.WeatherForecastsState
  & typeof WeatherForecastsStore.actionCreators;

function FetchData(props: WeatherForecastProps) {
  const { startDateIndex } = useParams<{ startDateIndex?: string }>();
  const startIndex = parseInt(startDateIndex || '0', 10) || 0;

  useEffect(() => {
    props.requestWeatherForecasts(startIndex);
  }, [startIndex]);

  const renderForecastsTable = () => {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Date</th>
            <th>Temp. (C)</th>
            <th>Temp. (F)</th>
            <th>Summary</th>
          </tr>
        </thead>
        <tbody>
          {props.forecasts.map((forecast: WeatherForecastsStore.WeatherForecast) =>
            <tr key={forecast.date}>
              <td>{forecast.date}</td>
              <td>{forecast.temperatureC}</td>
              <td>{forecast.temperatureF}</td>
              <td>{forecast.summary}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  };

  const renderPagination = () => {
    const prevStartDateIndex = startIndex - 5;
    const nextStartDateIndex = startIndex + 5;

    return (
      <div className="d-flex justify-content-between">
        <Link className='btn btn-outline-secondary btn-sm' to={`/fetch-data/${prevStartDateIndex}`}>Previous</Link>
        {props.isLoading && <span>Loading...</span>}
        <Link className='btn btn-outline-secondary btn-sm' to={`/fetch-data/${nextStartDateIndex}`}>Next</Link>
      </div>
    );
  };

  return (
    <React.Fragment>
      <h1 id="tabelLabel">Weather forecast</h1>
      <p>This component demonstrates fetching data from the server and working with URL parameters.</p>
      {renderForecastsTable()}
      {renderPagination()}
    </React.Fragment>
  );
}

export default connect(
  (state: ApplicationState) => state.weatherForecasts,
  WeatherForecastsStore.actionCreators
)(FetchData);

