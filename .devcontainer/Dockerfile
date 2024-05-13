FROM rzippo/texlive-basic:may-2024

USER root

## Add project-specific texlive packages
RUN tlmgr install \
    pgf \
    pgfplots \
    xcolor \
    preview

# Add powershell 7.4, using deb package
RUN wget https://github.com/PowerShell/PowerShell/releases/download/v7.4.2/powershell_7.4.2-1.deb_amd64.deb && \
    dpkg -i powershell_7.4.2-1.deb_amd64.deb

## Cleanup
RUN rm powershell_7.4.2-1.deb_amd64.deb

USER texlive

# Add dotnet 8.0, using user-local install
RUN wget https://dot.net/v1/dotnet-install.sh -O /home/texlive/dotnet-install.sh && \
    chmod +x /home/texlive/dotnet-install.sh && \
    /home/texlive/dotnet-install.sh --channel 8.0

ENV PATH="/home/texlive/.dotnet:${PATH}"
ENV DOTNET_ROOT="/home/texlive/.dotnet"

## Cleanup
RUN rm /home/texlive/dotnet-install.sh
