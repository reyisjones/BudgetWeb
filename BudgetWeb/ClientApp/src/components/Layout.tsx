import React from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';

export default function Layout(props: { children?: React.ReactNode }) {
    return (
        <React.Fragment>
            <NavMenu />
            <Container>
                {props.children}
            </Container>
        </React.Fragment>
    );
}

