/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

import * as React from 'react';
import { Nav, INavProps, INavStyleProps } from 'office-ui-fabric-react/lib/Nav';

export class NavMenu extends React.Component<{}, {}> {
  constructor(props: INavProps, styleProps: INavStyleProps) {
    super(props, styleProps);
  }
   
  public render() {
    return (
      <Nav
        groups={[{
          links: [
            { name: 'Home', key: 'Home', url: '/' },
            { name: 'Pickers', key: 'Pickers', url: '/Picker' },
            { name: 'Groups', key: 'Groups', url: '/Groups' }
          ]
        }]}
      />
    );
  }
}
