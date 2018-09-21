load("-text", "../App/src/OptionPricing.UI.Console/bin/Debug/beta_graph_3-prices.matrix", "prices");
load("-text", "../App/src/OptionPricing.UI.Console/bin/Debug/beta_graph_3-steps.matrix", "steps");
load("-text", "../App/src/OptionPricing.UI.Console/bin/Debug/beta_graph_3.matrix", "betas");

%steps = flipud(steps);
%prices = flipud(prices);

mesh(prices, steps, betas);

%colorbar
axis("labelxyz");
xlabel("Precio Subyacente");
ylabel("Tiempo");
zlabel("Beta Opcion");
%axis([min(prices),max(prices),min(steps),max(steps),0,500],"manual","ticxyz");
axis([min(prices),200,min(steps),max(steps),0,100],"manual","ticxyz");
grid("on");


shading faceted
%shading interp


% Set max and min values for the color range
caxis([0 75]);
% rotate view, the first param is the horizontal rotation, the second is the asimuth
view(47, 35);

%print -color -dashes -dpng "beta3d-5.png";
