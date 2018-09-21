
prices = importdata('../App/src/OptionPricing.UI.Console/bin/Debug/beta_graph_3-prices.matrix', ' ', 5);
steps = importdata('../App/src/OptionPricing.UI.Console/bin/Debug/beta_graph_3-steps.matrix', ' ', 5);
betas = importdata('../App/src/OptionPricing.UI.Console/bin/Debug/beta_graph_3.matrix', ' ', 5);

prices =prices.data;
steps = steps.data;
betas = betas.data;

%load('-text', '../App/src/OptionPricing.UI.Console/bin/Debug/beta_graph_3-prices.matrix', 'prices');
%load('-text', '../App/src/OptionPricing.UI.Console/bin/Debug/beta_graph_3-steps.matrix', 'steps');
%load('-text', '../App/src/OptionPricing.UI.Console/bin/Debug/beta_graph_3.matrix', 'betas');

%steps = flipud(steps);
%prices = flipud(prices);

pl = mesh(prices, steps, betas);


%colorbar
%axis('labelxyz');
xlabel('Precio Subyacente');
ylabel('Tiempo');
zlabel('Beta Opcion');
%axis([min(prices),max(prices),min(steps),max(steps),0,500],'manual','ticxyz');
axis([0,500,min(steps),max(steps),0,500],'manual','ticxyz');
grid('on');
shading faceted

rotate(pl,[1 0 0], 25);

%h12 = surf(sp12, peaks(20));
%title('Rotation Around X-Axis')
%zdir = [1 0 0];
%rotate(h12,zdir,25)